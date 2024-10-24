import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class SpinnerService {
  busyReqCount = 0;
  private spinner = inject(NgxSpinnerService);

  startSpin() {
    this.busyReqCount++;
    this.spinner.show(undefined, {
      type: 'square-jelly-box',
      bdColor: 'rgba(255,255,255,0)',
      color: 'rgb(233, 84, 32)',
    });
  }

  stopSpin() {
    this.busyReqCount--;
    if (this.busyReqCount <= 0) {
      this.busyReqCount = 0;
      this.spinner.hide();
    }
  }
}
