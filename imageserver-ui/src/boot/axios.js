import axios from 'axios'

export default async ({ app, router, store, Vue }) => {
  Vue.prototype.$axios = axios;
}