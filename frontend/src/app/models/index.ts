export type UserRole = 'SuperAdmin' | 'HotelAdmin' | 'User';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  loyaltyPoints: number;
  hotelId?: string; // For HotelAdmin
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

export interface Hotel {
  id: string;
  name: string;
  description: string;
  address: string;
  city: string;
  country: string;
  starRating: number;
  imageUrl: string;
  amenities: string[];
}

export interface RoomCategory {
  id: string;
  hotelId: string;
  name: string;
  description: string;
  basePrice: number;
  maxOccupancy: number;
  imageUrl: string;
  amenities: string[];
}

export interface Room {
  id: string;
  hotelId: string;
  categoryId: string;
  roomNumber: string;
  isAvailable: boolean;
  category?: RoomCategory;
}

export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';

export interface Booking {
  id: string;
  userId: string;
  hotelId: string;
  roomId: string;
  checkInDate: string;
  checkOutDate: string;
  totalPrice: number;
  status: BookingStatus;
  loyaltyPointsEarned: number;
  user?: User;
  hotel?: Hotel;
  room?: Room;
}

export interface Payment {
  id: string;
  bookingId: string;
  amount: number;
  currency: string;
  paymentMethod: string;
  transactionId: string;
  status: 'Pending' | 'Completed' | 'Failed';
  createdAt: string;
}

export interface Promotion {
  id: string;
  code: string;
  discountPercentage: number;
  validFrom: string;
  validTo: string;
  isActive: boolean;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface DashboardStats {
  totalBookings: number;
  totalRevenue: number;
  activeUsers: number;
  activeHotels: number;
}
