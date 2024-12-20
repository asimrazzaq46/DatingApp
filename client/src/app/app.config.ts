import {
  ApplicationConfig,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TimeagoModule } from 'ngx-timeago';

import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtTokenInterceptor } from './_interceptors/jwt-token.interceptor';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { ModalModule } from 'ngx-bootstrap/modal';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        errorInterceptor,
        jwtTokenInterceptor,
        loadingInterceptor,
      ])
    ),
    provideAnimations(),
    provideToastr({
      positionClass: 'toast-bottom-right',
    }),
    importProvidersFrom(
      NgxSpinnerModule,
      TimeagoModule.forRoot(),
      ModalModule.forRoot()
    ),
  ],
};
