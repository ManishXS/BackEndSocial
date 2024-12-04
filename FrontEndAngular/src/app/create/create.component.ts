import { Component, OnInit } from '@angular/core';
import { UploadService } from '../services/upload.service';
import { SharedService } from '../services/shared.service';
import { HttpEventType } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {
  selectedFile: File | null = null;
  uploadProgress: number = 0;
  userId: string | null = null;
  username: string | null = null;
  profilePic: string | null = null;
  caption: any;
  imgPreview: any;
  isUploading: boolean = false;
  uploadSuccess: boolean = false;
  errorMessage: string | null = null; // Added for error messages

  constructor(private uploadService: UploadService, private sharedService: SharedService, private router:Router) {}

  ngOnInit(): void {
    this.sharedService.getUserId().subscribe(userId => this.userId = userId);
    this.sharedService.getUsername().subscribe(username => this.username = username);
    this.sharedService.getProfilePic().subscribe(profilePic => this.profilePic = profilePic);
  }

  onFileSelected(event: any): void {
    this.errorMessage = null; // Clear previous error messages
    this.selectedFile = event.target.files[0] || null;
    if (this.selectedFile) {
      // Validate file size (45 MB = 45 * 1024 * 1024 bytes)
      if (this.selectedFile.size > 45 * 1024 * 1024) {
        this.errorMessage = 'File size must be 45 MB or smaller.';
        this.selectedFile = null; // Reset the selection
        return;
      }

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imgPreview = e.target.result;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  uploadFile(): void {
    if (this.selectedFile && this.userId && this.username) {
      this.isUploading = true; // Mark upload in progress
      const formData = new FormData();

      formData.append('file', this.selectedFile);
      formData.append('userId', this.userId);
      formData.append('userName', this.username);
      formData.append('fileName', this.selectedFile.name);
      formData.append('description', 'Sample description');
      formData.append('caption', this.caption);
      if (this.profilePic) {
        formData.append('profilePic', this.profilePic);
      }

      this.uploadService.uploadFile(formData).subscribe(event => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          this.uploadProgress = Math.round((100 * event.loaded) / event.total);
        } else if (event.type === HttpEventType.Response) {
          this.uploadSuccess = true;
          this.isUploading = false;
          setTimeout(() => {
            this.router.navigate(['/feeds']);
          }, 3000);

        }
      }, error => {
        console.error('Upload failed:', error);
        this.isUploading = false;
      });
    } else {
      console.error('User data or file is missing');
    }
  }

  cancelUpload(): void {
    this.isUploading = false;
    this.selectedFile = null;
    this.uploadProgress = 0;
    this.imgPreview = null;
    this.errorMessage = null;
  }
}
