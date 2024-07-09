import { Injectable } from '@angular/core';
import { ActiveToast, IndividualConfig, ToastrService } from 'ngx-toastr';
import { NotyfToast } from '../helpers/notyf.toast';
import { timeout } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class ToasterService {
  constructor(private toastr: ToastrService) { }

  showSuccessMessage(
    message: string,
    title?: string,
    option?: Partial<IndividualConfig>
  ) {
    const options = {
      toastComponent: NotyfToast,
      positionClass: 'toast-bottom-right',
      timeout: 3000,
      progressBar: true,
    };
    option = { ...options, ...option };
    return this.toastr.success(message, title, option);
  }

  succes(message: string, title?: string) {
    this.toastr.success(message, title);
  }

  error(message: string, title?: string) {
    this.toastr.error(message, title);
  }

  warning(message: string, title?: string) {
    this.toastr.warning(message, title);
  }

  info(message: string, title?: string) {
    this.toastr.info(message, title);
  }
}
