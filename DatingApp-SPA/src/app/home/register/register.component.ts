import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { validateConfig } from '@angular/router/src/config';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from 'src/app/_models/user';
import { routerNgProbeToken } from '@angular/router/src/router_module';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
@Input() valuesFromHome: any;
@Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
bsConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService,
    private alertify: AlertifyService, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    // this.registerForm = new FormGroup({
    //   name: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.minLength(4), Validators.maxLength(8), Validators.required]),
    //   confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
    this.bsConfig = {
      containerClass : 'theme-red'
    };
    this.createRegisterForm();
  }

createRegisterForm() {
  this.registerForm = this.fb.group({
    gender: ['male'],
    name: ['', Validators.required],
    knownAs: ['', Validators.required],
    dateOfBirth: [null, Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required],
    password: ['', [Validators.minLength(4), Validators.maxLength(8), Validators.required]],
    confirmPassword: ['', Validators.required]
  }, {validator : this.passwordMatchValidator});
}

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Success');
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    }
    // this.authService.register(this.model).subscribe(() => {
    //   this.alertify.success('successful registration');
    // }, error => {
    //   this.alertify.error('registration failed' + error);
    // });
    console.log(this.registerForm.value);
  }

passwordMatchValidator(g: FormGroup) {
  return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
}

  cancel() {
    this.cancelRegister.emit(false);
    this.alertify.message('cancel');
  }
}
