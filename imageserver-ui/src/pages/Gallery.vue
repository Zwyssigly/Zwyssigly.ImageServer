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
        <SecurityForm :endpoint="'security/' + $route.params.id" />
      </q-tab-panel>
      <q-tab-panel name="configuration">
        <SizeTable
          :value="configuration && configuration.sizes"
          :saveCallback="saveSizes" />
      </q-tab-panel>
      <q-tab-panel name="images">
        <ImageGutter :gallery="$route.params.id" ref="images" />
      </q-tab-panel>
    </q-tab-panels>
    <DeleteDialog :deleteCallback="del" ref="deleteDialog" />
  </div>
</template>

<script>
import SizeTable from 'components/SizeTable';
import ImageGutter from 'components/ImageGutter';
import DeleteDialog from 'components/DeleteDialog';
import SecurityForm from 'components/SecurityForm';
import { getFile } from 'src/helpers';

export default {
  name: 'PageGallery',
  components: { SizeTable, ImageGutter, DeleteDialog, SecurityForm },
  data () {
    return {
      configuration: null,
      tab: 'security',
      uploading: false
    }
  },
  computed: {
    id () { return this.$route.params.id; }
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    async saveSizes(sizes) {
      let response = await this.$axios.put('configurations/' + this.$route.params.id, { ...this.configuration, sizes });
      console.log(response);
      await this.refresh();
    },
    async refresh() {
      let response = await this.$axios.get('configurations/' + this.$route.params.id);
      this.configuration = response.data;
    },
    async del () {
      let response = await this.$axios.delete('configurations/' + this.$route.params.id);
      console.log(response);
      await this.$store.dispatch('app/refreshGalleries');
      await this.$router.replace({ name: 'index' });
    },
    async upload () {
      this.uploading = true;
      try {
        let file = await getFile();
        console.log(file);
        let response = await this.$axios.post(
          'images/' + this.$route.params.id, 
          file,
          { headers: { 'Content-Type': file.type } }
        );
        console.log(this.response);
        await this.$refs.images.refresh();
      } finally {
        this.uploading = false;
      }
    }
  }
}
</script>