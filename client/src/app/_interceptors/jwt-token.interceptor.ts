import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';

export const jwtTokenInterceptor: HttpInterceptorFn = (
  req,
  next
) => {
  const auth = inject(AccountService);

  if (auth.currentUser()) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${auth.currentUser()?.token}`,
      },
    });
  }

  return next(req);
};
