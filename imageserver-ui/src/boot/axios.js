import axios from 'axios'

export default async ({ app, router, store, Vue }) => {
  axios.defaults.baseURL = '/api/v1/'
  axios.defaults.headers.common['Authorization'] = localStorage.getItem('Authorization');
  Vue.prototype.$axios = axios;
}