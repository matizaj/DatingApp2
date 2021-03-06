import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { tokenKey } from '@angular/core/src/view';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { loadQueryList } from '@angular/core/src/render3/instructions';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
 login() {
   this.authService.login(this.model).subscribe(next => {
     this.alertify.success('logged in successfully');
   }, error => {
     this.alertify.error(error);
   }, () => {
     this.router.navigate(['/members']);
   });
 }

 loggedIn() {
  return this.authService.logged_In();
}

logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  this.authService.decodedToken = null;
  this.authService.currentUser = null;
  this.alertify.message('logged out');
  this.router.navigate(['/home']);
}

}
