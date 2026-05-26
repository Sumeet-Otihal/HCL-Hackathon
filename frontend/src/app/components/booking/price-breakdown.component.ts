import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-price-breakdown',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="space-y-4">
      <div class="flex justify-between text-navy-600">
        <span>₹{{ pricePerNight | number:'1.2-2' }} x {{ nights }} nights {{ roomCount > 1 ? ' x ' + roomCount + ' rooms' : '' }}</span>
        <span>₹{{ subtotal | number:'1.2-2' }}</span>
      </div>
      
      <div *ngIf="discount > 0" class="flex justify-between text-green-600 font-medium">
        <span>Discount {{ promoCode ? '(' + promoCode + ')' : '' }}</span>
        <span>- ₹{{ discount | number:'1.2-2' }}</span>
      </div>
      
      <div class="border-t border-slate-200 pt-4 flex justify-between text-xl font-bold text-navy-900">
        <span>Total Amount</span>
        <span>₹{{ total | number:'1.2-2' }}</span>
      </div>
      
      <p class="text-xs text-navy-500 text-center">
        You will earn <span class="text-gold-600 font-bold">{{ loyaltyPointsEarned }}</span> loyalty points on this booking.
      </p>
    </div>
  `
})
export class PriceBreakdownComponent {
  @Input() pricePerNight = 0;
  @Input() nights = 1;
  @Input() roomCount = 1;
  @Input() discount = 0;
  @Input() promoCode?: string;

  get subtotal(): number {
    return this.pricePerNight * this.nights * this.roomCount;
  }

  get total(): number {
    return this.subtotal - this.discount;
  }

  get loyaltyPointsEarned(): number {
    return Math.floor(this.total / 100);
  }
}
