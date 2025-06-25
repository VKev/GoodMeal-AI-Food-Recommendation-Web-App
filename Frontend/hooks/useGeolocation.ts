import { useState, useEffect, useCallback } from 'react';

export interface LocationData {
  latitude: number;
  longitude: number;
  accuracy?: number;
}

export interface GeolocationState {
  location: LocationData | null;
  loading: boolean;
  error: string | null;
  permission: 'prompt' | 'granted' | 'denied' | 'unavailable';
}

const LOCATION_STORAGE_KEY = 'goodmeal_location_permission';
const LOCATION_DATA_KEY = 'goodmeal_location_data';

export const useGeolocation = () => {
  const [state, setState] = useState<GeolocationState>({
    location: null,
    loading: false,
    error: null,
    permission: 'prompt'
  });

  // Load cached location data and permission status
  useEffect(() => {
    // Check stored permission status
    const storedPermission = localStorage.getItem(LOCATION_STORAGE_KEY);
    const storedLocationData = localStorage.getItem(LOCATION_DATA_KEY);

    if (storedPermission === 'granted' && storedLocationData) {
      try {
        const locationData = JSON.parse(storedLocationData);
        setState(prev => ({
          ...prev,
          location: locationData,
          permission: 'granted'
        }));
      } catch (error) {
        console.error('Error parsing stored location data:', error);
        localStorage.removeItem(LOCATION_DATA_KEY);
      }
    } else if (storedPermission) {
      setState(prev => ({
        ...prev,
        permission: storedPermission as GeolocationState['permission']
      }));
    }

    // Check browser support
    if (!navigator.geolocation) {
      setState(prev => ({ ...prev, permission: 'unavailable' }));
      localStorage.setItem(LOCATION_STORAGE_KEY, 'unavailable');
      return;
    }

    // Check current permission status if available
    if (navigator.permissions && storedPermission !== 'denied') {
      navigator.permissions.query({ name: 'geolocation' }).then((result) => {
        const currentPermission = result.state as GeolocationState['permission'];
        
        // Only update if different from stored permission
        if (currentPermission !== storedPermission) {
          setState(prev => ({ ...prev, permission: currentPermission }));
          localStorage.setItem(LOCATION_STORAGE_KEY, currentPermission);
          
          // If permission was granted and we don't have location, get it automatically
          if (currentPermission === 'granted' && !storedLocationData) {
            getCurrentPosition();
          }
        }
      });
    }
  }, []);

  const getCurrentPosition = useCallback(() => {
    if (!navigator.geolocation) {
      console.log('=== GEOLOCATION NOT SUPPORTED ===');
      setState(prev => ({
        ...prev,
        error: 'Geolocation is not supported by this browser.',
        permission: 'unavailable'
      }));
      localStorage.setItem(LOCATION_STORAGE_KEY, 'unavailable');
      return;
    }

    console.log('=== REQUESTING GEOLOCATION ===');
    console.log('Current state:', state);
    
    setState(prev => ({ ...prev, loading: true, error: null }));

    const options: PositionOptions = {
      enableHighAccuracy: true,
      timeout: 10000,
      maximumAge: 300000 // Cache for 5 minutes
    };

    console.log('Calling navigator.geolocation.getCurrentPosition with options:', options);

    navigator.geolocation.getCurrentPosition(
      (position) => {
        const location: LocationData = {
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
          accuracy: position.coords.accuracy
        };

        console.log('=== GEOLOCATION SUCCESS ===');
        console.log('Position received:', position);
        console.log('Location data:', location);
        console.log('===========================');

        // Save to localStorage
        localStorage.setItem(LOCATION_DATA_KEY, JSON.stringify(location));
        localStorage.setItem(LOCATION_STORAGE_KEY, 'granted');

        setState(prev => ({
          ...prev,
          location,
          loading: false,
          error: null,
          permission: 'granted'
        }));
      },
      (error) => {
        console.log('=== GEOLOCATION ERROR ===');
        console.log('Error code:', error.code);
        console.log('Error message:', error.message);
        console.log('Error details:', error);
        console.log('=========================');

        let errorMessage = 'Unable to retrieve your location.';
        let permission: GeolocationState['permission'] = 'denied';

        switch (error.code) {
          case error.PERMISSION_DENIED:
            errorMessage = 'Location access denied by user.';
            permission = 'denied';
            break;
          case error.POSITION_UNAVAILABLE:
            errorMessage = 'Location information is unavailable.';
            break;
          case error.TIMEOUT:
            errorMessage = 'The request to get user location timed out.';
            break;
          default:
            errorMessage = 'An unknown error occurred while retrieving location.';
            break;
        }

        // Save permission status
        localStorage.setItem(LOCATION_STORAGE_KEY, permission);

        setState(prev => ({
          ...prev,
          loading: false,
          error: errorMessage,
          permission
        }));
      },
      options
    );
  }, []);

  const requestLocation = useCallback(() => {
    getCurrentPosition();
  }, [getCurrentPosition]);

  const clearLocation = useCallback(() => {
    setState(prev => ({
      ...prev,
      location: null,
      error: null,
      permission: 'prompt'
    }));
    
    // Clear localStorage
    localStorage.removeItem(LOCATION_DATA_KEY);
    localStorage.removeItem(LOCATION_STORAGE_KEY);
  }, []);

  // Auto-request location if permission was previously granted
  useEffect(() => {
    const storedPermission = localStorage.getItem(LOCATION_STORAGE_KEY);
    const storedLocationData = localStorage.getItem(LOCATION_DATA_KEY);
    
    if (storedPermission === 'granted' && !storedLocationData && !state.loading) {
      getCurrentPosition();
    }
  }, [getCurrentPosition, state.loading]);

  return {
    ...state,
    requestLocation,
    clearLocation,
    hasLocation: !!state.location
  };
};
