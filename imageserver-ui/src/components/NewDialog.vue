<template>
  <q-dialog ref="dialog" persistent>
    <q-card>
      <q-card-section>
        <div class="text-h6">Create galllery</div>
      </q-card-section>
      <q-card-section class="row q-pt-none">
        <q-input
          dense
          v-model="name"
          autofocus
          counter
          :maxlength="20"
          :rules="[val => val.trim().length > 0 || 'At least one character is required']"
        />
      </q-card-section>
      <q-card-actions>
        <q-space />
        <q-btn icon="cancel" label="Abort" :disable="working" color="secondary" v-close-popup />
        <q-btn icon="delete_forever" color="negative" :loading="working" @click="doWork">
          Create
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
    saveCallback: Function
  },
  data: () => ({
    name: '',
    working: false
  }),
  methods: {
    show () {
      this.$refs.dialog.show();
    },
    async doWork () {
      this.working = true;
      try {
        await this.$props.saveCallback(this.name);
      } finally {
        this.$refs.dialog?.hide();
        this.working = false;
      }
    }
  }
}
</script>
