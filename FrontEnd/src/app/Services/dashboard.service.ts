import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConfigService } from '../shared/services/config-service';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private config: any;

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
  ) {
    this.configService.getConfig().subscribe((config) => {
      this.config = config;
    });
  }

  getDashboard(): Observable<any> {
    return this.http.get<any>(`${this.config.baseUrl}Dashboard`);
  }
}
