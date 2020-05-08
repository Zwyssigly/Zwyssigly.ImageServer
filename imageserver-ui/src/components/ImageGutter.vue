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
      <div class="col-xs-6 col-sm-4 col-md-3" v-for="image in images" :key="image.id">
        <q-card 
          class="cursor-pointer"
          @click="$router.push({ name: 'image', params: { gid: $route.params.id, iid: image.id } })"
        >
          <q-img
            :src="getSource(image)"
            ratio="1"
            :style="{ backgroundColor: '#' + image.fillColor }"
          >
            <div class="absolute-bottom text-caption row">
              <span class="id">{{image.id}}</span>
            </div>
          </q-img>
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
const pageSize = 12;

export default {
  name: 'ImageGutter',
  data () {
    return {
      currentPage: 1,
      totalPages: 1,
      images: []
    }
  },
  props: {
    gallery: String
  },
  async mounted () {
    await this.loadPage(1);
  },
  methods: {
    async loadPage (page) {
      let response = await this.$axios('images/' + this.$props.gallery + '?skip=' + (page-1)*pageSize + '&take=' + pageSize);
      this.totalPages = Math.ceil(response.data.totalItems/pageSize);
      this.currentPage = page;
      this.images = response.data.items;
    },
    refresh() {
      return this.loadPage(this.currentPage);
    },
    getSource (img) {
      return [
        '/api/v1/thumbnails/',
        this.$route.params.id,
        '/',
        encodeURIComponent(img.id),
        '/',
        encodeURIComponent(img.sizes[0].tag),
        '.',
        img.sizes[0].format
      ].join('');
    }
  }
}
</script>