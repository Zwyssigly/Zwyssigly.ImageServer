import axios from 'axios'

export function startRefresh (state) {
  state.galleries_loading = true;
}

export function endRefresh (state, galleries) {
  state.galleries_loading = false;
  state.galleries = galleries;
}

export function basicAuth(state, { username, password }) {
  state.authHeader = `Basic ${btoa(`${username}:${password}`)}`;
  axios.defaults.headers.common['Authorization'] = state.authHeader;
  localStorage.setItem('Authorization', state.authHeader);
}

