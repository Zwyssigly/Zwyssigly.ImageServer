import { ImageServerClient } from 'imageserver-client'

class Auth {  

  constructor(Vue) {
    this.Vue = Vue;

    let header = localStorage.getItem('Authorization')
    if (header?.length > 0) this.initialize(header);
  }

  initialize(header) {
    this.Vue.prototype.$client = new ImageServerClient({
      baseUrl: '',
      authorization: header
    });
    localStorage.setItem('Authorization', header);
  }

  isInitialized() {
    return this.Vue.prototype.$client ? true : false;
  }

  login(username, password) {
    let authHeader = `Basic ${btoa(`${username}:${password}`)}`;
    this.initialize(authHeader);
  }
}


export default async ({ app, router, store, Vue }) => {
  let auth = new Auth(Vue);
  Vue.prototype.$auth = auth;

  router.beforeEach((to, from, next) => {
    if (to.name !== 'login' && !auth.isInitialized()) next({ name: 'login' })
    else next()
  });
}