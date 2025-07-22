import { useQuery } from '@tanstack/react-query';
import { 
  findRestaurantsForFood, 
  getRestaurantDetail, 
  getRestaurantPhotos, 
  getRestaurantReviews,
  RestaurantPlace,
  RestaurantDetailResponse,
  RestaurantReview
} from '../services/RestaurantService';
import { FirebaseAuth } from '@/firebase/firebase';

// Query keys for caching
export const restaurantKeys = {
  all: ['restaurants'] as const,
  search: (params: { search: string; location?: string; lat?: string; lng?: string }) => 
    ['restaurants', 'search', params] as const,
  detail: (businessId: string, placeId: string) => 
    ['restaurants', 'detail', businessId, placeId] as const,
  photos: (businessId: string, placeId: string) => 
    ['restaurants', 'photos', businessId, placeId] as const,
  reviews: (businessId: string, placeId: string, cursor?: string) => 
    ['restaurants', 'reviews', businessId, placeId, cursor] as const,
  allReviews: (businessId: string, placeId: string) => 
    ['restaurants', 'allReviews', businessId, placeId] as const,
};

// Hook for searching restaurants
export const useRestaurantsSearch = (
  foodName: string,
  userCoords?: { latitude: number; longitude: number } | null,
  specificLocation?: string
) => {
  return useQuery({
    queryKey: restaurantKeys.search({
      search: foodName,
      location: specificLocation,
      lat: userCoords?.latitude?.toString(),
      lng: userCoords?.longitude?.toString(),
    }),
    queryFn: async () => {
      if (!foodName) {
        throw new Error('Food name is required');
      }
      
      const response = await findRestaurantsForFood(
        foodName,
        userCoords,
        specificLocation
      );
      
      if (!response.isSuccess || !response.value) {
        throw new Error('Failed to fetch restaurants');
      }
      
      return response.value as RestaurantPlace[];
    },
    enabled: !!foodName,
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 30 * 60 * 1000, // 30 minutes
  });
};

// Hook for restaurant detail
export const useRestaurantDetail = (businessId: string, placeId: string) => {
  return useQuery({
    queryKey: restaurantKeys.detail(businessId, placeId),
    queryFn: async () => {
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : '';
      const response = await getRestaurantDetail(idToken, businessId, placeId);
      
      if (!response.isSuccess || !response.value) {
        throw new Error('Failed to fetch restaurant details');
      }
      
      return response.value as RestaurantDetailResponse['value'];
    },
    enabled: !!(businessId && placeId),
    staleTime: 10 * 60 * 1000, // 10 minutes
    gcTime: 60 * 60 * 1000, // 1 hour
  });
};

// Hook for restaurant photos
export const useRestaurantPhotos = (businessId: string, placeId: string) => {
  return useQuery({
    queryKey: restaurantKeys.photos(businessId, placeId),
    queryFn: async () => {
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : '';
      const response = await getRestaurantPhotos(idToken, businessId, placeId);
      
      if (!response.isSuccess) {
        return []; // Return empty array on error instead of throwing
      }
      
      return response.value || [];
    },
    enabled: !!(businessId && placeId),
    staleTime: 30 * 60 * 1000, // 30 minutes
    gcTime: 2 * 60 * 60 * 1000, // 2 hours
  });
};

// Hook for single batch of reviews
export const useRestaurantReviews = (
  businessId: string, 
  placeId: string, 
  cursor?: string
) => {
  return useQuery({
    queryKey: restaurantKeys.reviews(businessId, placeId, cursor),
    queryFn: async () => {
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : '';
      const response = await getRestaurantReviews(idToken, businessId, placeId, cursor);
      
      if (!response.isSuccess) {
        return [];
      }
      
      return response.value || [];
    },
    enabled: !!(businessId && placeId),
    staleTime: 15 * 60 * 1000, // 15 minutes
    gcTime: 60 * 60 * 1000, // 1 hour
  });
};

// Hook for loading all reviews progressively
export const useAllRestaurantReviews = (businessId: string, placeId: string) => {
  return useQuery({
    queryKey: restaurantKeys.allReviews(businessId, placeId),
    queryFn: async () => {
      let allReviews: RestaurantReview[] = [];
      let currentCursor: string | null = null;
      let hasMore = true;
      let batchCount = 0;
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : '';
      
      // Load reviews in batches until we get all of them
      while (hasMore && batchCount < 50) { // Safety limit
        batchCount++;
        const response = await getRestaurantReviews(idToken, businessId, placeId, currentCursor || undefined);
        
        if (response.isSuccess && response.value && response.value.length > 0) {
          allReviews = [...allReviews, ...response.value];
          
          // Check if there are more reviews to load
          const lastReview = response.value[response.value.length - 1];
          if (lastReview && lastReview.review_cursor) {
            currentCursor = lastReview.review_cursor;
            // Small delay between requests
            await new Promise(resolve => setTimeout(resolve, 100));
          } else {
            hasMore = false;
          }
        } else {
          hasMore = false;
        }
      }
      
      return allReviews;
    },
    enabled: !!(businessId && placeId),
    staleTime: 15 * 60 * 1000, // 15 minutes
    gcTime: 60 * 60 * 1000, // 1 hour
  });
};
