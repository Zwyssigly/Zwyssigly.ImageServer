<template>
  <q-list :dense="dense" style="width:100%">
    <q-item v-if="loading">
      <q-spinner />
    </q-item>
    <q-item
      v-for="gallery in galleries"
      :key="gallery"
      clickable
      :to="{ name: 'gallery', params: { id: gallery } }"
    >
      <q-item-section>
        {{gallery}}
      </q-item-section>
    </q-item>
    <q-item
      clickable
      @click="$refs.dialog.show()"
      v-if="!loading"
    >
      <q-item-section>
        <i>Add new</i>
        <NewDialog :saveCallback="createGallery" ref="dialog" />
      </q-item-section>
    </q-item>
  </q-list>
</template>

<script>
import NewDialog from 'components/NewDialog';

export default {
  name: 'GalleryList',
  components: { NewDialog },
  props: {
    dense: Boolean
  },
  computed: {
    galleries () { return this.$store.state.app.galleries; }, 
    loading () { return this.$store.state.app.galleries_loading; }
  },
  methods: {
    async createGallery (name) {
      var response = await this.$axios.put('configurations/' + name, {
        name,
        sizes: []
      });
      console.log(response);
      await this.$store.dispatch('app/refreshGalleries');
      await this.$router.push({ name: 'gallery', params: { id: name }});
    }
  }
}
</script>