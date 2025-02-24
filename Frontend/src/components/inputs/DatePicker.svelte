<script lang="ts">
	import CalendarIcon from 'lucide-svelte/icons/calendar';
	import { type DateValue } from '@internationalized/date';
	import { cn } from '$lib/utils.js';
	import { Button } from '$lib/components/ui/button';
	import { Calendar } from '$lib/components/ui/calendar';
	import * as Popover from '$lib/components/ui/popover/index';

	export let id: string | undefined;
	export let value: DateValue | undefined = undefined;
</script>

<Popover.Root>
	<Popover.Trigger asChild let:builder>
		<Button
			variant="outline"
			class={cn(
				'w-[280px] justify-start rounded border-gray-500 bg-white text-left font-normal',
				!value && 'text-muted-foreground'
			)}
			builders={[builder]}
		>
			<CalendarIcon class="mr-2 h-4 w-4" />
			{value ? value.toDate('utc').toDateString() : 'Pick a date'}
		</Button>
	</Popover.Trigger>
	<Popover.Content class="w-auto p-0 ">
		<Calendar {id} bind:value initialFocus />
	</Popover.Content>
</Popover.Root>
