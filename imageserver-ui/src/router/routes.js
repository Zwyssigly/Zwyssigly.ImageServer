
const routes = [
  {
    path: '/login',
    name: 'login',
    component: () => import('pages/Login.vue'),
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { name: 'index', path: '', component: () => import('pages/Index.vue') },
      { name: 'gallery', path: 'g/:id', component: () => import('pages/Gallery.vue') },
      { name: 'image', path: 'g/:gid/:iid', component: () => import('pages/Image.vue') },
      { name: 'global', path: 'gs', component: () => import('pages/Global.vue') }
    ]
  }
]

// Always leave this as last one
if (process.env.MODE !== 'ssr') {
  routes.push({
    path: '*',
    component: () => import('pages/Error404.vue')
  })
}

export default routes
