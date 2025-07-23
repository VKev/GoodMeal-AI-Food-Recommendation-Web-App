const RESTAURANT_API_BASE = 'http://localhost:2406/api/restaurant';
import { FirebaseAuth } from "@/firebase/firebase";

export interface RestaurantPlace {
  business_id: string;
  name: string;
  full_address: string;
  phone_number: string | null;
  latitude: number;
  longitude: number;
  rating: number;
  review_count: number;
  place_link: string;
  place_id: string;
  website: string | null;
  timezone: string;
  city: string;
  state: string | null;
  price_level: string | null;
  types: string[];
  description: string[];
  photos: Array<{
    src: string;
    max_width: number;
    max_height: number;
    min_width: number;
    min_height: number;
  }>;
  working_hours: Record<string, string[]>;
}

export interface NearbyRestaurantsRequest {
  query: string;
  Lat: number;
  Lng: number;
}

export interface NearbyRestaurantsResponse {
  value: RestaurantPlace[];
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export interface GeocodingRequest {
  query: string;
}

export interface GeocodingResponse {
  value: {
    address: string;
    latitude: number;
    longitude: number;
    timezone: string;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

// Restaurant Detail API interfaces and functions
export interface RestaurantDetailRequest {
  Business_id: string;
  Place_id: string;
}

export interface RestaurantDetailResponse {
  value: {
    business_id: string;
    name: string;
    full_address: string;
    full_address_array: string[];
    phone_number: string;
    latitude: number;
    longitude: number;
    review_count: number;
    rating: number;
    timezone: string;
    website: string | null;
    place_id: string;
    place_link: string;
    types: string[];
    price_level: string | null;
    plus_code: string;
    cid: string;
    is_claimed: boolean;
    working_hours: Record<string, string[]>;
    city: string;
    state: string;
    description: string[];
    details: Record<string, string[]>;
    photos: Array<{
      src: string;
      max_width: number;
      max_height: number;
      min_width: number;
      min_height: number;
    }>;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export interface RestaurantReviewRequest {
  Business_id: string;
  Place_id: string;
  cursor?: string;
}

export interface RestaurantReview {
  user_name: string;
  user_total_reviews: number;
  user_total_photos: number;
  user_avatar: string;
  user_link: string;
  iso_date: string;
  iso_date_timestamp: number;
  iso_date_of_last_edit: string;
  iso_date_of_last_edit_timestamp: number;
  review_id: string;
  review_time: string;
  review_timestamp: number;
  review_link: string;
  review_text: string;
  review_photos: any[];
  business_response_text: string | null;
  review_services: Record<string, any>;
  translations: Record<string, string>;
  review_rate: number;
  review_cursor: string | null;
}

export interface RestaurantReviewResponse {
  value: RestaurantReview[];
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export interface RestaurantPhotoRequest {
  Business_id: string;
  Place_id: string;
}

export interface RestaurantPhotoResponse {
  value: Array<{
    src: string;
    max_width: number;
    max_height: number;
    min_width: number;
    min_height: number;
  }>;
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

// Get nearby restaurants using coordinates
export const getNearbyRestaurants = async (
  idToken: string,
  query: string,
  lat: number,
  lng: number
): Promise<NearbyRestaurantsResponse> => {
  try {
    const response = await fetch(`${RESTAURANT_API_BASE}/place/nearby`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${idToken}`
      },
      body: JSON.stringify({
        query,
        Lat: lat,
        Lng: lng
      } as NearbyRestaurantsRequest)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching nearby restaurants:', error);
    throw error;
  }
};

// Get coordinates from location name/query
export const getLocationCoordinates = async (
  idToken: string,
  query: string
): Promise<GeocodingResponse> => {
  try {
    if (!idToken) throw new Error("No idToken provided");
    const response = await fetch(`${RESTAURANT_API_BASE}/place/geocoding`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${idToken}`
      },
      body: JSON.stringify({ query })
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching location coordinates:', error);
    throw error;
  }
};

// Main function to find restaurants - handles both cases
export const findRestaurantsForFood = async (
  foodName: string,
  userLocation?: { latitude: number; longitude: number } | null,
  specificLocation?: string
): Promise<NearbyRestaurantsResponse> => {
  try {
    let coordinates: { lat: number; lng: number };
    // Lấy idToken một lần ở đầu hàm
    const user = FirebaseAuth.currentUser;
    const idToken = user ? await user.getIdToken() : '';

    // console.log("findRestaurantsForFood - user:", user);
    // console.log("findRestaurantsForFood - idToken:", idToken);

    if (specificLocation && specificLocation !== 'null' && specificLocation.trim()) {
      // Case 1: User specified a location (e.g., "phở ở Hà Nội")
      // console.log('Using specific location:', specificLocation);
      // console.log('token:', idToken);

      const geoResponse = await getLocationCoordinates(idToken, specificLocation);

      if (!geoResponse.isSuccess || !geoResponse.value) {
        throw new Error('Failed to get coordinates for specified location');
      }

      coordinates = {
        lat: geoResponse.value.latitude,
        lng: geoResponse.value.longitude
      };
    } else if (userLocation) {
      // Case 2: Use user's current location
      coordinates = {
        lat: userLocation.latitude,
        lng: userLocation.longitude
      };
    } else {
      // Case 3: Default to Vietnam (fallback)
      console.log('Using default location (Vietnam)');
      const geoResponse = await getLocationCoordinates(idToken, 'Vietnam');

      if (!geoResponse.isSuccess || !geoResponse.value) {
        // Fallback coordinates for Vietnam (Ho Chi Minh City)
        coordinates = {
          lat: 10.8231,
          lng: 106.6297
        };
      } else {
        coordinates = {
          lat: geoResponse.value.latitude,
          lng: geoResponse.value.longitude
        };
      }
    }

    // Now get nearby restaurants using the coordinates
    return await getNearbyRestaurants(idToken, foodName, coordinates.lat, coordinates.lng);

  } catch (error) {
    console.error('Error in findRestaurantsForFood:', error);
    throw error;
  }
};

// Get restaurant detail
export const getRestaurantDetail = async (
  idToken: string,
  businessId: string,
  placeId: string
): Promise<RestaurantDetailResponse> => {
  try {
    if (!idToken) throw new Error("No idToken provided");
    const response = await fetch(`${RESTAURANT_API_BASE}/place/detail`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${idToken}`
      },
      body: JSON.stringify({
        Business_id: businessId,
        Place_id: placeId
      } as RestaurantDetailRequest)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching restaurant detail:', error);
    throw error;
  }
};

// Get restaurant reviews
export const getRestaurantReviews = async (
  idToken: string,
  businessId: string,
  placeId: string,
  cursor?: string
): Promise<RestaurantReviewResponse> => {
  try {
    if (!idToken) throw new Error("No idToken provided");
    const requestBody: RestaurantReviewRequest = {
      Business_id: businessId,
      Place_id: placeId
    };

    if (cursor) {
      requestBody.cursor = cursor;
    }

    const response = await fetch(`${RESTAURANT_API_BASE}/place/review`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${idToken}`
      },
      body: JSON.stringify(requestBody)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching restaurant reviews:', error);
    throw error;
  }
};

// Get restaurant photos
export const getRestaurantPhotos = async (
  idToken: string,
  businessId: string,
  placeId: string
): Promise<RestaurantPhotoResponse> => {
  try {
    if (!idToken) throw new Error("No idToken provided");
    const response = await fetch(`${RESTAURANT_API_BASE}/place/photo`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${idToken}`
      },
      body: JSON.stringify({
        Business_id: businessId,
        Place_id: placeId
      } as RestaurantPhotoRequest)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching restaurant photos:', error);
    throw error;
  }
};
