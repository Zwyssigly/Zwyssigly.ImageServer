<template>
  <div class="q-pa-md" v-if="image">
    <q-breadcrumbs class="q-mb-sm">
      <q-breadcrumbs-el label="Galleries" :to="{ name: 'index' }" />
      <q-breadcrumbs-el :label="$route.params.gid" :to="{ name: 'gallery', params: { id: $route.params.gid } }" />
      <q-breadcrumbs-el :label="image.id" />
    </q-breadcrumbs>
    <div class="row q-my-md" v-if="image">
      <div class="col-auto q-pr-sm">
        <Thumbnail
          class="shadow-1"
          style="minHeight:100px"
          :gallery="$route.params.gid"
          :value="thumbnail"
          width="256px"
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
    <ThumbnailTable :gallery="$route.params.gid" :image="image" />
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
import Thumbnail from 'components/Thumbnail';
import { getFile } from 'src/helpers';

export default {
  name: 'PageImage',
  components: { ThumbnailTable, Thumbnail, DeleteDialog, ColorSquare },
  data () {
    return {
      image: null,
      thumbnail: null,
      uploading: false
    }
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    async refresh() {
      let gallery = this.$client.getGallery(this.$route.params.gid);
      this.image = (await gallery.getImages([this.$route.params.iid]))[0];
      this.thumbnail = (await gallery.resolveThumbnails([this.image.id], { minWidth: 256 }))[0];
    },
    async del() {
      await this.$client.getGallery(this.$route.params.gid).deleteImages([this.$route.params.iid]); 
      await this.$router.replace({ name: 'gallery', params: { id: this.$route.params.gid } });
    },
    async replace() {
      this.uploading = true;
      try {
        let file = await getFile();
        await this.$client.getGallery(this.$route.params.gid).replaceImage(this.$route.params.iid, file);
        await this.refresh();
      } finally {
        this.uploading = false;
      }
    },
    getSource (img) {
      return this.$client.getGallery(this.$route.params.gid)
        .getThumbnailDataUrl(img.id, img.sizes[0].tag, img.sizes[0].format, img.rowVersion);
    }
  }
}
</script>