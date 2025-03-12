<script lang="ts">
	import { cn } from 'src/lib/utils';

	export let href: string;
	export let isActive: boolean = false;
</script>

<a
	{href}
	class={cn(
		'flex w-full items-center justify-start gap-2 px-4 py-3 text-lg',
		isActive ? 'bg-blue-100 font-semibold' : 'hover:bg-blue-50'
	)}
>
	<slot />
</a>
