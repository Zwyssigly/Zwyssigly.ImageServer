import axios from 'axios'

export async function refreshGalleries (context) {
  context.commit('startRefresh');
  var response = await axios.get('configurations');
  context.commit('endRefresh', response.data);
}

