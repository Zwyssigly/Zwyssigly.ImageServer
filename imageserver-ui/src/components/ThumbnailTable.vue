<template>
  <q-table
    :data="rows"
    :columns="columns"
    :loading="!image"
    :pagination.sync="pagination"
  >
    <template v-slot:body-cell-actions="props">
      <q-td :props="props">    
        <q-btn icon="save_alt" dense flat round @click="download(props.row)" />    
      </q-td>
    </template>
  </q-table>
</template>

<script>
export default {
  name: 'ThumbnailTable',
  props: {
    gallery: String,
    image: Object
  },
  computed: {
    rows () { return this.$props.image.sizes; }
  },
  data () {
    return {
      columns: [
        { name: 'tag', label: 'Tag', align: 'left', field: row => row.tag, sortable: true },
        { name: 'width', label: 'Width', align: 'right', field: row => row.width, sortable: true },
        { name: 'height', label: 'Height', align: 'right', field: row => row.height, sortable: true },
        { name: 'aspectRatio', label: 'Aspect ratio', align: 'left', field: row => row.aspectRatio, sortable: true  },
        { name: 'crop', label: 'Crop', align: 'left', field: row => row.cropStrategy, sortable: true },
        { name: 'format', label: 'Format', align: 'left', field: row => row.format, sortable: true },
        { name: 'quality', label: 'Quality', align: 'right', field: row => row.quality, sortable: true },
        { name: 'duplicateOf', label: "Dupl. of", align: 'left', field: row => row.duplicateOf, sortable: true },
        { name: 'actions', label: '', align: 'left' },
      ],
      pagination: { rowsPerPage: 10 }
    }
  },
  methods: {
    async download (size) {
      let url = await this.$client.getGallery(this.$props.gallery).getThumbnailDataUrl({
        id: this.$props.image.id,
        tag: size.tag,
        format: size.format,
        rowVersion: this.$props.image.rowVersion
      });

      let anchor = document.createElement('a')
      anchor.href = url;
      anchor.download = `${this.$props.image.id}$${size.tag}.${size.format}`
      anchor.click();
    }
  }
}
</script>