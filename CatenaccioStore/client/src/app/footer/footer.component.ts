import { Component } from '@angular/core';
import { faFacebook, faInstagram, faTiktok, faYoutube } from '@fortawesome/free-brands-svg-icons';
import { faAddressBook, faAddressCard, faEnvelope, faMap, faPhone } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  facebook = faFacebook;
  instagram = faInstagram;
  tiktok = faTiktok;
  youtube = faYoutube;
  address = faMap;
  mail = faEnvelope;
  phone = faPhone;

}
