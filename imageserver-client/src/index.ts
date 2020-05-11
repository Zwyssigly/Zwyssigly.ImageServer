import axios from 'axios';
import { AxiosInstance } from 'axios';

export interface ImageServerOptions {
  baseUrl: string
  authorization: string
}

export class ImageServerClient {

  private _axios: AxiosInstance;
  private _galleries: { [id: string] : ImageServerGallery } = {}

  constructor(options: ImageServerOptions) {
    this._axios = axios.create({
      baseURL: options.baseUrl + '/api/v1/',
      headers: { 'Authorization': options.authorization }
    });
  }

  getGallery(galleryName: string) : ImageServerGallery {
    if (!this._galleries[galleryName]) {
      this._galleries[galleryName] = new ImageServerGallery(this._axios, galleryName);
    }
    return this._galleries[galleryName]
  }

  getSecurity() : Promise<SecurityConfiguration> {
    return this._axios.get('security-global').then(resp => resp.data);
  }

  setSecurity(configuration: SecurityConfiguration) : Promise<void> {
    return this._axios.put('security-global', configuration);
  }

  getHealth() : Promise<HealthStatus> {
    return this._axios.get('health').then(resp => resp.data);
  }

  listGalleries() : Promise<string[]> {
    return this._axios.get('galleries').then(resp => resp.data);
  }

  newGallery(galleryName: string) : Promise<void> {
    return this._axios.post(`galleries/${galleryName}`);
  }

  deleteGallery(galleryName: string) : Promise<void> {
    return this._axios.delete(`galleries/${galleryName}`);
  }
}

export class ImageServerGallery {

  constructor(private _axios: AxiosInstance, private _galleryName: string) {

  }

  getConfiguration() : Promise<Configuration> {
    return this._axios.get(`configurations/${this._galleryName}`).then(resp => resp.data);
  }

  setConfiguration(configuration: Configuration) : Promise<void> {
    return this._axios.put(`configurations/${this._galleryName}`, configuration);
  }

  getSecurity() : Promise<SecurityConfiguration> {
    return this._axios.get(`security/${this._galleryName}`).then(resp => resp.data);
  }

  setSecurity(configuration: SecurityConfiguration) : Promise<void> {
    return this._axios.put(`security/${this._galleryName}`, configuration);
  }

  getImages(ids: string[]) : Promise<Image> {
    return this._axios.get(`images/${this._galleryName}/${ids.join(',')}`).then(resp => resp.data);
  }

  uploadImage(file: Blob) : Promise<Image> {
    return this._axios.post(`images/${this._galleryName}`, file, { headers: { 'Content-Type': file.type } }).then(resp => resp.data);
  }

  replaceImage(id: string, file: Blob) : Promise<Image> {
    return this._axios.put(`images/${this._galleryName}/${id}`, file, { headers: { 'Content-Type': file.type } }).then(resp => resp.data);
  }

  deleteImages(ids: string[]) : Promise<Image> {
    return this._axios.delete(`images/${this._galleryName}/${ids.join(',')}`);
  }

  listImages(skip: number, take: number) : Promise<Image[]> {
    return this._axios.get(`images/${this._galleryName}?skip=${skip}&take=${take}`).then(resp => resp.data);
  }

  getThumbnailBlob(thumbnail: { id: string, rowVersion: number, tag: string, format: string }) : Promise<Blob> {
    let url = this.internalThumbnailUrl(thumbnail);
    return this._axios.get(url, { responseType: 'blob' }).then(resp => resp.data)
  }

  private internalThumbnailUrl(thumbnail: { id: string, rowVersion: number, tag: string, format: string }) : string {
    return `thumbnails/${this._galleryName}/${thumbnail.id}/v${thumbnail.rowVersion}/${thumbnail.tag}.${thumbnail.format}`      
  }

  getThumbnailUrl(thumbnail: { id: string, rowVersion: number, tag: string, format: string }) : string {
    return `${this._axios.defaults.baseURL}${this.internalThumbnailUrl(thumbnail)}`;
  }

  async getThumbnailDataUrl(thumbnail: { id: string, rowVersion: number, tag: string, format: string }) : Promise<string> {
    let blob = await this.getThumbnailBlob(thumbnail);
    return new Promise((resolve, reject) => {
      let fileReader = new FileReader();
      fileReader.onerror = () => reject(fileReader.error);
      fileReader.onload = () => resolve(<string>fileReader.result);
      fileReader.readAsDataURL(blob);
    });
  }

  async resolveThumbnails(ids: string[], options: { tag?: string, minWidth?: number, minHeight?: number }) {
    let parts = [`thumbnails/${this._galleryName}/${ids.join(',')}`];

    if (options.tag) parts.push(`tag=${options.tag}`);
    if (options.minWidth) parts.push(`minWidth=${options.minWidth}`);
    if (options.minHeight) parts.push(`minHeight=${options.minHeight}`);

    let url = parts.length < 3
      ? parts.join('?')
      : `${parts[0]}?${parts[1]}&${parts.slice(2).join('&')}`;

    return this._axios.get(url).then(resp => resp.data);
  }
}

export interface HealthStatus {
  version: string,
  message: string
}

export interface Configuration {
  avoidDuplicates: Boolean,
  sizes: SizeConfiguration[]
}

export interface SizeConfiguration {
  tag: string,
  maxWidth?: number,
  maxHeight?: number,
  format: string,
  quality: number,
  crop?: CropConfiguration
}

export interface CropConfiguration {
  aspectRatio: string,
  cropStrategy: string,
  color?: string
}

export interface SecurityConfiguration {
  accounts: AccountConfiguration[]
}

export interface AccountConfiguration {
  name: string,
  type: string,
  password?: string
  permissions: string[]
}

export interface Image {
  id: string,
  rowVersion: number,
  uploadedAt: string,
  fillColor: string,
  edgeColor: string,
  md5: string,
  meta: string,
  sizes: ImageSize[],
}

export interface ImageSize {
  tag: string,
  aspectRatio: string,
  width: number,
  height: number,
  cropStrategy?: string,
  format: string,
  quality: number,
  duplicateOf?: string
}

export interface ResolvedThumbnail {
  id: string,
  rowVersion: number,
  fillColor: string,
  edgeColor: string,
  tag: string,
  format: string,
  width: number,
  height: number
}
