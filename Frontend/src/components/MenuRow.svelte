<script lang="ts">
	export let href: string;
</script>

<a {href} class="flex w-full items-center justify-start gap-2 px-4 py-3 text-lg hover:bg-blue-50">
	<slot />
</a>
