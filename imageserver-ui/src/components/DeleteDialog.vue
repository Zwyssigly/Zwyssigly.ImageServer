<template>
  <q-dialog ref="dialog" persistent>
    <q-card>
      <q-card-section class="row items-center">
        <span>Do you really want to delete '{{item && nameField(item)}}'?</span>
      </q-card-section>
      <q-card-actions>
        <q-space />
        <q-btn icon="cancel" label="Abort" :disable="working" color="secondary" v-close-popup />
        <q-btn icon="delete_forever" color="negative" :loading="working" @click="doWork">
          Delete
          <q-spinner slot="loading" />
        </q-btn>
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script>
export default {
  name: 'DeleteDialog',
  props: {
    nameField: {
      type: Function,
      default: q => q
    },
    deleteCallback: Function
  },
  data: () => ({
    working: false,
    item: null
  }),
  methods: {
    show (item) {
      this.item = item;
      this.$refs.dialog.show();
    },
    async doWork () {
      this.working = true;
      try {
        await this.$props.deleteCallback(this.item);
      } finally {
        this.$refs.dialog?.hide();
        this.working = false;
      }
    }
  }
}
</script>
