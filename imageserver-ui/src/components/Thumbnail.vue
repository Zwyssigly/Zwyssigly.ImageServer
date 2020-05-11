<template>
  <q-img
    :src="source"
    :ratio="ratio"
    :width="width"
    :style="$props.value ? { backgroundColor: '#' + $props.value.fillColor } : {}"
  >
    <q-inner-loading :showing="!source" />
    <div class="absolute-bottom text-caption row" v-if="$props.label">
      <span class="id">{{$props.value.id}}</span>
    </div>
  </q-img>
</template>

<script>
export default {
  name: 'Thumbnail',
  props: {
    gallery: String,
    value: Object,
    label: Boolean,
    ratio: String,
    width: String
  },
  data () {
    return { source: null };
  },
  updated () {
    this.refresh();
  },
  mounted () {
    this.refresh();
  },
  methods: {
    async refresh () {
      this.source = this.$props.value 
        ? await this.$client.getGallery(this.$props.gallery).getThumbnailDataUrl(this.$props.value)
        : null;
    }
  }
}
</script>