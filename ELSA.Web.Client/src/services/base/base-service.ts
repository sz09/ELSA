import { HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Direction } from '../../utilities/page.utilities';

export class BaseService {
  protected host: string = environment.applicationApi;
  protected API_VERSION: number = environment.apiVersion;
  constructor() {
  }
  protected getRequestHeaders(): { headers: HttpHeaders | { [header: string]: string | string[]; } } {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': `application/vnd.iman.v1+json, application/json, text/plain, */*`,
      'Access-Control-Allow-Origin': 'https://localhost:7058',
      'Access-Control-Allow-Credentials': 'true',
      'Cross-Origin': '*'
    });

    return { headers: headers };
  }

  protected getRequestHeadersWithoutContentTypes(): { headers: HttpHeaders | { [header: string]: string | string[]; } } {
    let headers = new HttpHeaders({
      'Accept': `application/vnd.iman.v1+json, application/json, text/plain, */*`,
      'Access-Control-Allow-Origin': 'https://localhost:7058',
      'Access-Control-Allow-Credentials': 'true',
      'Cross-Origin': '*'
    });

    return { headers: headers };
  }

  protected getBodyData(orderBy: string, direction: Direction, page: number, pageSize: number){
    return JSON.stringify({
      'orderBy': orderBy,
      'orderDirection': direction,
      'page': page,
      'pageSize': pageSize,
    });
  }
}
