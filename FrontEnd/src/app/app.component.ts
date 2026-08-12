import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HeaderComponent } from './shared/Components/header/header.component';
import { authInterceptor } from './shared/Interceptors/authInterceptor';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent], // تأكد من أنك استوردت RouterOutlet

  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'Technical_Assessment_ElectroPi';
}
