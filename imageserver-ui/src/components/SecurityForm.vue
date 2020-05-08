<template>
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
    endpoint: String
  },
  data () {
    return { configuration: null };
  },
  async mounted () {
    await this.refresh();
  },
  methods: {
    async saveAccounts(accounts) {
      let response = await this.$axios.put(this.$props.endpoint, { ...this.configuration, accounts });
      console.log(response);
      await this.refresh();
    },
    refresh() {
      return this.$axios.get(this.$props.endpoint).then(resp => {
        this.configuration = resp.data;
      }).catch(error => {
        if (error.response.status === 404)
          this.configuration = { accounts: [] };
        return Promise.reject(error);
      });
    }
  }
}
</script>