<template>
  <div class="q-pa-md" v-if="image">
    <q-breadcrumbs class="q-mb-sm">
      <q-breadcrumbs-el label="Galleries" :to="{ name: 'index' }" />
      <q-breadcrumbs-el :label="$route.params.gid" :to="{ name: 'gallery', params: { id: $route.params.gid } }" />
      <q-breadcrumbs-el :label="image.id" />
    </q-breadcrumbs>
    <div class="row q-my-md" v-if="image">
      <div class="col-auto q-pr-sm">
        <q-img
          :src="getSource(image)"
          width="256px"
          class="shadow-1"
          :style="{ backgroundColor: '#' + image.fillColor, minHeight: '100px' }"
        />
      </div>
      <div class="col">
        <div class="row">
          <label class="col-4 text-caption">Original MD5</label>
          <span class="col-8">{{image.md5}}</span>
        </div>
        <div class="row">
          <label class="col-4 text-caption">Uploaded at</label>
          <span class="col-8">{{new Date(image.uploadedAt).toLocaleString()}}</span>
        </div>
        <div class="row">
          <label class="col-4 text-caption">Fill color</label>
          <ColorSquare class="col-8" :value="image.fillColor" />
        </div>
        <div class="row">
          <label class="col-4 text-caption">Edge color</label>
          <ColorSquare class="col-8" :value="image.edgeColor" />
        </div>
        <div class="row">
          <label class="col-4 text-caption">Row version</label>
          <span class="col-8">v{{image.rowVersion}}</span>
        </div>
      </div>
    </div>
    <ThumbnailTable :value="image.sizes" :gallery="$route.params.gid" :image="$route.params.iid" />
    <div class="row q-mt-sm">
      <q-space />
      <q-btn label="Replace" icon="camera_alt" @click="replace" :loading="uploading" class="q-mr-md" />
      <q-btn color="negative" label="Delete" icon="delete" @click="$refs.dialog.show()" />
    </div>
    <DeleteDialog :name="image.id" ref="dialog" :deleteCallback="del" />
  </div>
</template>

<script>
import ThumbnailTable from 'components/ThumbnailTable';
import DeleteDialog from 'components/DeleteDialog';
import ColorSquare from 'components/ColorSquare';
import { getFile } from 'src/helpers';

export default {
  name: 'PageImage',
  components: { ThumbnailTable, DeleteDialog, ColorSquare },
  data () {
    return {
      image: null,
      uploading: false
    }
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    async refresh() {
      let response = await this.$axios.get(this.buildUrl());
      this.image = response.data;
    },
    async del() {
      let response = await this.$axios.delete(this.buildUrl());      
      await this.$router.replace({ name: 'gallery', params: { id: this.$route.params.gid } });
    },
    async replace() {
      this.uploading = true;
      try {
        let file = await getFile();
        let response = await this.$axios.put(
          this.buildUrl(), 
          file,
          { headers: { 'Content-Type': file.type } }
        );
        console.log(this.response);
        await this.refresh();
      } finally {
        this.uploading = false;
      }
    },
    buildUrl() {
      return [
        'images/',
        encodeURIComponent(this.$route.params.gid),
        '/',
        encodeURIComponent(this.$route.params.iid)
      ].join('');
    },
    getSource (img) {
      console.log(img);
      return [
        '/api/v1/thumbnails/',
        this.$route.params.gid,
        '/',
        encodeURIComponent(img.id),
        '/v',
        encodeURIComponent(img.rowVersion),
        '/',
        encodeURIComponent(img.sizes[0].tag),
        '.',
        img.sizes[0].format
      ].join('');
    }
  }
}
</script>