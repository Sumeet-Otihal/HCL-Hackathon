import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [ngClass]="classes">
      <ng-content></ng-content>
    </span>
  `
})
export class BadgeComponent {
  @Input() variant: 'default' | 'success' | 'warning' | 'error' | 'info' = 'default';
  @Input() class = '';

  get classes(): string {
    const base = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium';
    const variants: Record<string, string> = {
      default: 'bg-navy-100 text-navy-800',
      success: 'bg-green-100 text-green-800',
      warning: 'bg-yellow-100 text-yellow-800',
      error: 'bg-red-100 text-red-800',
      info: 'bg-blue-100 text-blue-800',
    };

    return `${base} ${variants[this.variant]} ${this.class}`;
  }
}
