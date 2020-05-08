<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="menu"
          aria-label="Menu"
          @click="leftDrawerOpen = !leftDrawerOpen"
        />

        <q-toolbar-title>
          Image Server UI
        </q-toolbar-title>

        <div> 
          <span v-if="version">v{{ version }}</span>
          <q-spinner v-if="!version" />
        </div>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      show-if-above
      bordered
    >
      <q-list>
        <q-item-label header>
          Galleries
        </q-item-label>
        <q-item>
          <GalleryList dense />
        </q-item>
        <q-item :to="{ name: 'global' }">
          <q-item-section>Security</q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view :key="$route.fullPath" />
    </q-page-container>
  </q-layout>
</template>

<script>
import GalleryList from 'components/GalleryList'

export default {
  name: 'MainLayout',
  components: {
    GalleryList
  },
  mounted () {
    this.$store.dispatch('app/refreshGalleries');
    this.$axios.get('health').then(resp => {
      this.version = resp.data.version;
    });
  },
  data () {
    return {
      leftDrawerOpen: false,
      version: null
    }
  }
}
</script>
