<template>
  <q-table
    :data="rows"
    :columns="columns"
    :loading="!value"
    :pagination.sync="pagination"
    @row-click="(_, r) => openThumbnail(r)"
  >
    <template v-slot:body-cell-actions="props">
      <q-td :props="props">        
      </q-td>
    </template>
  </q-table>
</template>

<script>
export default {
  name: 'ThumbnailTable',
  props: {
    value: Array,
    gallery: String,
    image: String,
  },
  computed: {
    rows () { return this.$props.value; }
  },
  data () {
    return {
      columns: [
        { name: 'tag', label: 'Tag', align: 'left', field: row => row.tag, sortable: true },
        { name: 'width', label: 'Width', align: 'right', field: row => row.width, sortable: true },
        { name: 'height', label: 'Height', align: 'right', field: row => row.height, sortable: true },
        // { name: 'quality', label: 'Quality', align: 'right', field: row => row.quality, sortable: true },
        { name: 'aspectRatio', label: 'Aspect ratio', align: 'left', field: row => row.aspectRatio, sortable: true  },
        { name: 'crop', label: 'Crop', align: 'left', field: row => row.cropStrategy, sortable: true },
        { name: 'format', label: 'Format', align: 'left', field: row => row.format, sortable: true },
        { name: 'actions', label: '', align: 'left' }
      ],
      pagination: { rowsPerPage: 10 }
    }
  },
  methods: {
    openThumbnail (size) {
      var url = [
        '/api/v1/thumbnails/',
        this.$props.gallery,
        '/',
        this.$props.image,
        '/',
        size.tag,
        '.',
        size.format
      ].join('');
      window.open(url);
    }
  }
}
</script>