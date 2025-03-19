<script lang="ts">
	export let field: {
		value: any;
	};
</script>

<a href={field?.value} target="_blank" class="text-blue-500">{field?.value}</a>
