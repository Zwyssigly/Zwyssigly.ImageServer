<template>
  <div class="q-pa-md" :key="id">
    <div class="row q-mb-sm">
      <q-breadcrumbs class="q-mb-sm">
        <q-breadcrumbs-el label="Galleries" :to="{ name: 'index' }" />
        <q-breadcrumbs-el :label="$route.params.id" />
      </q-breadcrumbs>
      <q-space />
      <q-btn label="Upload" icon="camera_alt" @click="upload" :loading="uploading" />
      <q-btn
        label="Delete"
        icon="delete"
        color="negative"
        class="q-ml-sm"
        @click="$refs.deleteDialog.show($route.params.id)"
      />
    </div>
    <!--q-separator /-->
    <q-tabs
      v-model="tab"
      dense
      align="justify"
      narrow-indicator
    >
      <q-tab name="security" label="Security" />
      <q-tab name="configuration" label="Configuration" />
      <q-tab name="images" label="Images" />      
    </q-tabs>
    <q-separator />
    <q-tab-panels v-model="tab" animated>
      <q-tab-panel name="security">
        <SecurityForm :gallery="$route.params.id" />
      </q-tab-panel>
      <q-tab-panel name="configuration">
        <ConfigurationForm :gallery="$route.params.id" />
      </q-tab-panel>
      <q-tab-panel name="images">
        <ImageGutter :gallery="$route.params.id" ref="images" />
      </q-tab-panel>
    </q-tab-panels>
    <DeleteDialog :deleteCallback="del" ref="deleteDialog" />
  </div>
</template>

<script>
import ConfigurationForm from 'components/ConfigurationForm';
import ImageGutter from 'components/ImageGutter';
import DeleteDialog from 'components/DeleteDialog';
import SecurityForm from 'components/SecurityForm';
import { getFile } from 'src/helpers';

export default {
  name: 'PageGallery',
  components: { ConfigurationForm, ImageGutter, DeleteDialog, SecurityForm },
  data () {
    return {
      tab: 'security',
      uploading: false
    }
  },
  computed: {
    id () { return this.$route.params.id; }
  },
  methods: {
    async del () {
      let response = await this.$client.deleteGallery(this.$route.params.id);
      console.log(response);
      await this.$store.dispatch('app/refreshGalleries', this.$client);
      await this.$router.replace({ name: 'index' });
    },
    async upload () {
      this.uploading = true;
      try {
        let file = await getFile();
        console.log(file);
        let response = await this.$client.getGallery(this.$route.params.id).uploadImage(file);
        console.log(this.response);
        await this.$refs.images.refresh();
      } finally {
        this.uploading = false;
      }
    }
  }
}
</script>