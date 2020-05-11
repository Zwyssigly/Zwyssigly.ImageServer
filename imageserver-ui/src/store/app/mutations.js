export function startRefresh (state) {
  state.galleries_loading = true;
}

export function endRefresh (state, galleries) {
  state.galleries_loading = false;
  state.galleries = galleries;
}

