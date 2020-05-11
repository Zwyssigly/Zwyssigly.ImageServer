<template>
  <div>
    <SizeTable 
      :value="configuration && configuration.sizes"
      :saveCallback="saveSizes"
    />
    <q-separator class="q-mt-md" />
    <q-form class="q-ma-md">
      <div class="row text-h6">Processing</div>
      <div class="row q-mt-md">
        <q-toggle
          v-model="changes.avoidDuplicates"
          label="Avoid thumbnail duplicates"
        />
      </div>
      <div class="row q-mt-md">
        <q-btn color="primary" @click="save()" label="Save" :loading="saving" />
      </div>
    </q-form>
  </div>
</template>

<script>
import SizeTable from 'components/SizeTable'

export default {
  name: 'SecurityForm',
  components: { SizeTable },
  props: {
    gallery: String
  },
  data () {
    return { 
      configuration: null,
      changes: {},
      saving: false,
    };
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    async saveSizes(sizes) {
      let response = await this.$client.getGallery(this.$props.gallery).setConfiguration({ ...this.configuration, sizes });
      console.log(response);
      await this.refresh();
    },
    refresh () {
      return this.$client.getGallery(this.$props.gallery).getConfiguration().then(data => {
        this.configuration = data;
        this.changes = { 
          avoidDuplicates: this.configuration.avoidDuplicates 
        };
      }).catch(error => {
        return Promise.reject(error);
      });
    },
    async save () {
      this.saving = true;
      try {
        let response = await this.$client.getGallery(this.$props.gallery).setConfiguration({ ...this.configuration, ...this.changes });
        console.log(response);
        await this.refresh();
      } finally {
        this.saving = false;
      }
    }
  }
}
</script>