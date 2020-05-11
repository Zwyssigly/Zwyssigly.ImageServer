export async function refreshGalleries (context, client) {
  context.commit('startRefresh');
  var galleries = await client.listGalleries();
  context.commit('endRefresh', galleries);
}

