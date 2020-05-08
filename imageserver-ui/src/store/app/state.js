export default function () {
  return {
    galleries: [],
    galleries_loading: false,
    authHeader: localStorage.getItem('Authorization')
  }
}
