import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';

export interface AppConfig {
  baseUrl: string;
}

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  private readonly configUrl = 'assets/Configurations/config.json';

  private readonly config$: Observable<AppConfig>;

  constructor(private http: HttpClient) {
    this.config$ = this.http
      .get<AppConfig>(this.configUrl)
      .pipe(shareReplay(1));
  }

  getConfig(): Observable<AppConfig> {
    return this.config$;
  }
}
