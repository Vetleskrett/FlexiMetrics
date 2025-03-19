<script lang="ts">
	export let field: {
		value: any | undefined;
	};
</script>

<p>
	<span class="text-nowrap">{new Date(field.value).toDateString()}</span>
	<span>{new Date(field.value).toLocaleTimeString()}</span>
</p>
