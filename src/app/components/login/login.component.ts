import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { ResetPasswordService } from 'src/app/services/reset-password.service';
import { ToasterService } from 'src/app/services/toaster.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  type: string = 'password';
  isText: boolean = false;
  eyeIcon: string = 'fa-eye-slash';
  loginForm!: FormGroup;
  public resetPasswordEmail!: string;
  public isValidEmail!: boolean;

  constructor(
    private formBuilder: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: ToasterService,
    private userStore: UserStoreService,
    private resetService: ResetPasswordService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? (this.eyeIcon = 'fa-eye') : (this.eyeIcon = 'fa-eye-slash');
    this.isText ? (this.type = 'text') : (this.type = 'password');
  }
  onLogin() {
    if (this.loginForm.valid) {
      this.auth.login(this.loginForm.value).subscribe({
        next: (res) => {
          this.loginForm.reset();
          this.auth.storeToken(res.accessToken);
          this.auth.storeRefreshToken(res.refreshToken);
          const tokenPayload = this.auth.decodeToken();
          this.toast.succes('Login Successful');
          this.redirectBasedOnRole(tokenPayload.role);
        },
        error: (err) => {
          this.toast.warning('Invalid Account');
        },
      });
    } else {
      Object.values(this.loginForm.controls).forEach(control => {
        control.markAsTouched();
      });
    }
  }

  redirectBasedOnRole(role: string) {
    switch (role) {
      case 'Admin':
        this.router.navigate(['dashboard']);
        break;
      case 'Teacher':
        this.router.navigate(['teacher-dashboard']);
        break;
      case 'Student':
        this.router.navigate(['student-dashboard']);
        break;
      default:
        this.router.navigate(['login']);
        break;
    }
  }

  checkValidEmail(event: string) {
    const value = event;
    const emailRegEx = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,3}$/;
    this.isValidEmail = emailRegEx.test(value);
    return this.isValidEmail;
  }
  confirmToSend() {
    if (this.checkValidEmail(this.resetPasswordEmail)) {
      console.log(this.resetPasswordEmail);

      this.resetService.sendResetPasswordLink(this.resetPasswordEmail)
        .subscribe({
          next: (res) => {
            this.toast.showSuccessMessage('Reset Password Email Sent');
            this.resetPasswordEmail = "";
            const buttonRef = document.getElementById('closeBtn');
            buttonRef?.click();
          },

          error: (err) => {
            this.toast.warning('Something went wrong');
          }
        })
    }
  }
}
