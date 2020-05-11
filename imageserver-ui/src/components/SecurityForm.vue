<template style="width:100%">
  <AccountTable 
    :value="configuration && configuration.accounts"
    :saveCallback="saveAccounts"
  />
</template>

<script>
import AccountTable from 'components/AccountTable'

export default {
  name: 'SecurityForm',
  components: { AccountTable },
  props: {
    gallery: String
  },
  data () {
    return { configuration: null };
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    saveAccounts(accounts) {
      return this.save({ accounts });
    },
    async save(configuration) {
      let promise = this.$props.gallery
        ? this.$client.getGallery(this.$props.gallery).setSecurity({ ...this.configuration, ...configuration })
        : this.$client.setSecurity({ ...this.configuration, ...configuration });

      let response = await promise;
      console.log(response);
      await this.refresh();
    },
    async refresh() {
      let promise = this.$props.gallery
        ? this.$client.getGallery(this.$props.gallery).getSecurity()
        : this.$client.getSecurity();

      this.configuration = await promise;
    }
  }
}
</script>