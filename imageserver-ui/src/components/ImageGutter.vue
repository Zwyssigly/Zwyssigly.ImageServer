<style lang="scss">
  .id {        
    width: 100%;
    white-space: nowrap;
    overflow: hidden;
    text-emphasis: ellipsis;
  }
</style>

<template>
  <div>
    <div class="row justify-center">
      <q-pagination
        :value="currentPage"
        color="purple"
        :max="totalPages"
        :max-pages="6"
        :boundary-numbers="true"
        @input="val => loadPage(val)"
      />
    </div>  
    <div class="row q-col-gutter-md q-my-sm">
      <div class="col-xs-6 col-sm-4 col-md-3" v-for="thumbnail in thumbnails" :key="thumbnail.id">
        <q-card 
          class="cursor-pointer"
          @click="$router.push({ name: 'image', params: { gid: $route.params.id, iid: thumbnail.id } })"
        >
          <Thumbnail ratio="1" :value="thumbnail" label :gallery="$props.gallery" />          
        </q-card>
      </div>
    </div>
    <div class="row justify-center">
      <q-pagination
        :value="currentPage"
        color="purple"
        :max="totalPages"
        :max-pages="6"
        :boundary-numbers="true"
        @input="val => loadPage(val)"
      />
    </div>  
  </div>
</template>

<script>
import Thumbnail from 'components/Thumbnail';

const pageSize = 12;

export default {
  name: 'ImageGutter',
  components: { Thumbnail },
  data () {
    return {
      currentPage: 1,
      totalPages: 1,
      thumbnails: []
    }
  },
  props: {
    gallery: String
  },
  async mounted () {
    await this.loadPage(1);
  },
  methods: {
    async loadPage (currentPage) {
      let page = await this.$client.getGallery(this.$props.gallery).listImages((currentPage-1)*pageSize, pageSize);

      this.totalPages = Math.ceil(page.totalItems/pageSize);
      this.currentPage = currentPage;

      let ids = page.items.map(i => i.id);
      this.thumbnails = await this.$client.getGallery(this.$props.gallery).resolveThumbnails(ids, { minWidth: 256, minHeight: 256 });
    },
    refresh() {
      return this.loadPage(this.currentPage);
    }
  }
}
</script>