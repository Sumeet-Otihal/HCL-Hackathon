import { format } from 'date-fns';

export const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
  }).format(amount);
};

export const formatDate = (date: string | Date) => {
  return format(new Date(date), 'dd MMM yyyy');
};
