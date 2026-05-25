import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-amenity-icon',
  standalone: true,
  imports: [CommonModule, ],
  template: `
    
  `
})
export class AmenityIconComponent implements OnInit {
  @Input() amenity!: string;
  @Input() class = 'h-4 w-4 mr-1';

  iconName = 'wifi'; // default

  // Inject required icons here so they are available in the module context if needed,
  // but lucide-angular components use the name input when configured globally.
  // Actually, for standalone we need to import specific icons or all.
  
  ngOnInit() {
    const map: Record<string, string> = {
      'WiFi': 'wifi',
      'Pool': 'waves',
      'Spa': 'sprout',
      'Gym': 'dumbbell',
      'Restaurant': 'utensils',
      'Parking': 'car',
      'TV': 'tv',
      'AC': 'wind',
      'Breakfast': 'coffee',
    };
    
    this.iconName = map[this.amenity] || 'wifi';
  }
}
