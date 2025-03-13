<script lang="ts">
	import { Separator } from 'src/lib/components/ui/separator';
	import type { StudentAnalysis } from 'src/types/';
	import StudentAnalysisCard from './StudentAnalysisCard.svelte';

	export let studentAnalyses: StudentAnalysis[];
</script>

<div class="flex w-full flex-col gap-4">
	<div class="flex items-center gap-2">
		<div class="grow">
			<Separator class="h-[2px] rounded-lg bg-gray-300" />
		</div>
		<div class="flex items-center gap-[1px]">
			<img
				width="28"
				height="28"
				src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
				alt="knowledge-sharing"
			/>
			<h2 class="text-lg font-semibold">Analyses</h2>
		</div>
		<div class="grow">
			<Separator class="h-[2px] rounded-lg bg-gray-300" />
		</div>
	</div>
	<div class="grid w-full grid-cols-3 gap-8">
		{#each studentAnalyses as studentAnalysis}
			<StudentAnalysisCard {studentAnalysis} />
		{/each}
	</div>
</div>
