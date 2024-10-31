import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SpinnerService } from '../_services/spinner.service';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const spinner = inject(SpinnerService);
  spinner.startSpin();
  return next(req).pipe(
    delay(500),
    finalize(() => spinner.stopSpin())
  );
};
