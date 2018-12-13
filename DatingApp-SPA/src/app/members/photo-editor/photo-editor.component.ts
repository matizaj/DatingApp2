import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';
import { Photo } from './../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
@Input() photos: Photo[];
@Output() getMemberPhotoChange = new EventEmitter<string>();
uploader: FileUploader;
hasBaseDropZoneOver = false;
baseUrl = environment.apiUrl;
currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }
  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          DateAdded: res.DateAdded,
          Description: res.Description,
          IsMain: res.IsMain
        };
        this.photos.push(photo);
      }
    };

  }

  setMainPhoto(photo: Photo) {
    this.userService.setmainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
    console.log(this.photos);
    this.currentMain = this.photos.filter(p => p.IsMain === true)[0];
    this.currentMain.IsMain = false;
    photo.IsMain = true;
    this.authService.changeMemberPhoto(photo.url);
    this.authService.currentUser.photoUrl = photo.url;
    localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
     }, error => {
    this.alertify.error(error);
  });
}
deletePhoto(id: number) {
  this.alertify.confirm('Are you sure you want to delete this photo', () => {
    this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      this.alertify.success('photo deleted!');
    }, error => {
      this.alertify.error(error);
    });
  });
}
}