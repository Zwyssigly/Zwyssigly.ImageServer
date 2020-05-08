<template>
  <q-page class="q-ma-md">
    <div class="row text-h1 q-mb-lg">
      <div class="col-auto q-mr-md">
        <q-img
          alt="Logo"
          src="statics/icons/icon-128x128.png"          
          height="1em"
          width="1em"
          ratio="1"
        />
      </div>
      <div class="col">Image Server</div>
    </div>
    <q-separator />
    <div class="row q-mt-lg">
      <div class="col-4">
        <div class="text-subtitle2 q-mb-sm">Galleries</div>
        <GalleryList dense />
      </div>
      <div class="col-8">
        <div class="text-subtitle2 q-mb-sm">Links</div>
        <q-list dense>
          <q-item clickable @click="open('https://github.com/Zwyssigly/Zwyssigly.ImageServer/blob/master/README.md')" >
            <q-item-section avatar><q-avatar icon="notes"/></q-item-section>
            <q-item-section>Documentation</q-item-section>            
          </q-item>
          <q-item clickable @click="open('https://github.com/Zwyssigly/Zwyssigly.ImageServer')">
            <q-item-section avatar><q-avatar icon="code"/></q-item-section>
            <q-item-section>Source Code</q-item-section>
          </q-item>
          <q-item clickable @click="open('https://github.com/Zwyssigly/Zwyssigly.ImageServer')">
            <q-item-section avatar><q-avatar icon="save"/></q-item-section>
            <q-item-section>Docker Image</q-item-section>
          </q-item>
        </q-list>
      </div>
    </div>
  </q-page>
</template>

<script>
import GalleryList from 'components/GalleryList';

export default {
  name: 'PageIndex',
  components: { GalleryList },
  methods: {
    open (url) {
      window.open(url);
    }
  }
}
</script>
