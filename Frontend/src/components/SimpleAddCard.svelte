<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
    import Plus from 'lucide-svelte/icons/plus';
    import Button from '$lib/components/ui/button/button.svelte';

	export let headline: string, inputString: string, inputType: string, actionString: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
    <Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/add--v1.png"
				alt="add"
			/>
			<Card.Title class="ml-4 text-3xl">{headline}</Card.Title>
		</div>
    </Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
            <Separator class="w-full" />
			<div class="flex items-center px-6 text-sm font-bold m-8">
				<div>
					<label class="pl-2" for="AddInput">{inputString}</label>
					<input
						id="AddInput"
						class="h-9 mr-2 p-2 border-2 border-black rounded-md"
						placeholder={inputString}
						type={inputType}
					/>
				</div>
                <Button
                    class="flex h-9 flex-row justify-between gap-2 bg-button-green pl-2 pr-3 hover:bg-button-green-hover mt-auto"
                    >
					<Plus/>
                    <p>{actionString}</p>
                </Button>
			</div>
		</div>
	</Card.Content>
</Card.Root>