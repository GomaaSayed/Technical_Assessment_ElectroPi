import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../Services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  dashboard: any = null;

  isLoading = true;
  errorMessage = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getDashboard().subscribe({
      next: (response) => {
        console.log('Dashboard response:', response);

        this.dashboard = response;
        this.isLoading = false;
      },

      error: (error) => {
        console.error('Failed to load dashboard:', error);

        this.errorMessage =
          error.error?.message || 'Failed to load dashboard data.';

        this.isLoading = false;
      },
    });
  }
}
